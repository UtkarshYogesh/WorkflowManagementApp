<template>
  <div class="auth-container">
    <div class="auth-box">
      <div class="auth-header">
        <h1>Create Account</h1>
        <p class="auth-subtitle">Join our task management platform</p>
      </div>

      <form @submit.prevent="handleRegister" class="auth-form">
        <div class="form-group">
          <label for="username">Username</label>
          <input
            id="username"
            v-model="formData.username"
            type="text"
            placeholder="Enter your username"
            required
            class="form-input"
          />
        </div>

        <div class="form-group">
          <label for="email">Email</label>
          <input
            id="email"
            v-model="formData.email"
            type="email"
            placeholder="Enter your email"
            required
            class="form-input"
          />
        </div>

        <div class="form-group">
          <label for="password">Password</label>
          <input
            id="password"
            v-model="formData.password"
            type="password"
            placeholder="Enter your password"
            required
            class="form-input"
          />
        </div>

        <div class="form-group">
          <label for="confirmPassword">Confirm Password</label>
          <input
            id="confirmPassword"
            v-model="confirmPassword"
            type="password"
            placeholder="Confirm your password"
            required
            class="form-input"
          />
        </div>

        <div v-if="error" class="error-message">
          {{ error }}
        </div>

        <button type="submit" :disabled="isLoading" class="auth-button">
          {{ isLoading ? 'Creating Account...' : 'Create Account' }}
        </button>
      </form>

      <div class="auth-footer">
        <p>Already have an account?</p>
        <RouterLink to="/login" class="auth-link">Login here</RouterLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import { useAuth } from '../composables/useAuth'

const formData = ref({
  username: '',
  email: '',
  password: '',
})

const confirmPassword = ref('')
const { register, isLoading, error } = useAuth()

const handleRegister = async () => {
  if (formData.value.password !== confirmPassword.value) {
    alert('Passwords do not match!')
    return
  }

  try {
    await register({
      username: formData.value.username,
      email: formData.value.email,
      password: formData.value.password,
    })
  } catch (err) {
    console.error('Registration error:', err)
  }
}
</script>

<style scoped>
.auth-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #090b13 0%, #1e293b 100%);
  padding: 20px;
}

.auth-box {
  background: #0f172a;
  border: 1px solid rgba(148, 163, 184, 0.2);
  border-radius: 12px;
  padding: 40px;
  width: 100%;
  max-width: 400px;
  box-shadow: 0 20px 60px rgba(15, 23, 42, 0.3);
}

.auth-header {
  margin-bottom: 32px;
  text-align: center;
}

.auth-header h1 {
  font-size: 28px;
  font-weight: 700;
  color: #e2e8f0;
  margin: 0 0 8px 0;
}

.auth-subtitle {
  color: #cbd5e1;
  font-size: 14px;
  margin: 0;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 18px;
  margin-bottom: 24px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 500;
  color: #cbd5e1;
}

.form-input {
  padding: 10px 14px;
  background: #1e293b;
  border: 1px solid rgba(148, 163, 184, 0.3);
  border-radius: 8px;
  color: #e2e8f0;
  font-size: 14px;
  transition: all 0.2s ease;
}

.form-input:focus {
  outline: none;
  background: #1e293b;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-input::placeholder {
  color: #64748b;
}

.error-message {
  padding: 12px;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.3);
  border-radius: 8px;
  color: #fca5a5;
  font-size: 14px;
}

.auth-button {
  padding: 12px;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s ease;
  margin-top: 8px;
}

.auth-button:hover:not(:disabled) {
  background: #2563eb;
  transform: translateY(-1px);
}

.auth-button:disabled {
  background: #64748b;
  cursor: not-allowed;
  opacity: 0.6;
}

.auth-footer {
  text-align: center;
  border-top: 1px solid rgba(148, 163, 184, 0.1);
  padding-top: 20px;
}

.auth-footer p {
  color: #cbd5e1;
  font-size: 14px;
  margin: 0 0 8px 0;
}

.auth-link {
  color: #3b82f6;
  text-decoration: none;
  font-weight: 600;
  transition: color 0.2s ease;
}

.auth-link:hover {
  color: #60a5fa;
  text-decoration: underline;
}
</style>
