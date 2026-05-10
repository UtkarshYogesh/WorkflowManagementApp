import { ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  loginUser,
  registerUser,
  logoutUser,
  getStoredUser,
  getToken,
  isAuthenticated as checkIsAuthenticated,
  type LoginRequest,
  type RegisterRequest,
  type AuthResponse,
} from '../services/authApi'
import { setAccessToken, setUser } from '../services/tokenStore'
import { setRefreshTokenCookie } from '../services/cookieHelper'

const user = ref<Omit<AuthResponse, 'accessToken' | 'refreshToken'> | null>(getStoredUser())
const isLoading = ref(false)
const error = ref<string | null>(null)

export const useAuth = () => {
  const router = useRouter()

  const register = async (data: RegisterRequest) => {
    isLoading.value = true
    error.value = null
    try {
      const response = await registerUser(data)
      const { accessToken, refreshToken, ...userData } = response.data

      // Store tokens and user in memory
      setAccessToken(accessToken)
      setUser(userData)
      setRefreshTokenCookie(refreshToken)

      user.value = userData

      // Redirect to login
      router.push('/login')
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Registration failed'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const login = async (data: LoginRequest) => {
    isLoading.value = true
    error.value = null
    try {
      const response = await loginUser(data)
      const { ...userData } = response.data

      // Tokens are already stored by loginUser function
      user.value = userData

      // Redirect to home/dashboard
      router.push('/')
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Login failed'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const logout = () => {
    logoutUser()
    user.value = null
    error.value = null
    router.push('/login')
  }

  const isAuthenticated = () => {
    return checkIsAuthenticated() && !!user.value
  }

  return {
    user,
    isLoading,
    error,
    register,
    login,
    logout,
    isAuthenticated,
  }
}
