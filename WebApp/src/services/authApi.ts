import api from './api'
import { setAccessToken, getAccessToken, setUser, getUser, clearTokens } from './tokenStore'
import { setRefreshTokenCookie, clearRefreshTokenCookie } from './cookieHelper'
import { isTokenExpired } from './jwtHelper'

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  username: string
  email: string
  password: string
}

export interface AuthResponse {
  id: string
  username: string
  email: string
  accessToken: string
  refreshToken: string
}

// Register a new user
export const registerUser = async (data: RegisterRequest) => {
  return api.post<AuthResponse>('/auth/register', data)
}

// Login user
export const loginUser = async (data: LoginRequest) => {
  const response = await api.post<AuthResponse>('/auth/login', data)

  // Store tokens after login
  if (response.data) {
    setAccessToken(response.data.accessToken)
    setUser({
      id: response.data.id,
      username: response.data.username,
      email: response.data.email
    })
    setRefreshTokenCookie(response.data.refreshToken)
  }

  return response
}

// Logout user (clear tokens)
export const logoutUser = () => {
  clearTokens()
  clearRefreshTokenCookie()
}

// Get stored token (from memory)
export const getToken = () => {
  return getAccessToken()
}

// Get stored user (from memory)
export const getStoredUser = () => {
  return getUser()
}

// Check if user is authenticated
export const isAuthenticated = () => {
  const token = getAccessToken()
  return !!token && !isTokenExpired(token)
}

// Refresh token API call
export const refreshToken = async () => {
  try {
    const response = await api.post<{ token: string; refreshToken: string }>('/auth/refresh')

    if (response.data) {
      setAccessToken(response.data.token)
      setRefreshTokenCookie(response.data.refreshToken)
      return response.data.token
    }
  } catch (error) {
    console.error('Token refresh failed:', error)
    clearTokens()
    clearRefreshTokenCookie()
    throw error
  }
}
