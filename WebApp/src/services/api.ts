import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios'
import { getAccessToken, setAccessToken, setUser, clearTokens } from './tokenStore'
import { getRefreshTokenCookie, setRefreshTokenCookie, clearRefreshTokenCookie } from './cookieHelper'
import { isTokenExpired } from './jwtHelper'

const api = axios.create({
  baseURL: 'https://localhost:7062/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

let isRefreshing = false
let failedQueue: any[] = []

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error)
    } else {
      prom.resolve(token)
    }
  })

  failedQueue = []
}

// Function to refresh access token
const refreshAccessToken = async () => {
  try {
    const refreshToken = getRefreshTokenCookie()
    if (!refreshToken) {
      throw new Error('No refresh token available')
    }

    const response = await axios.post<{
      id: string
      username: string
      email: string
      role?: string
      accessToken: string
      refreshToken: string
    }>(
      'https://localhost:7062/api/auth/refresh',
      JSON.stringify(refreshToken),
      {
        headers: {
          'Content-Type': 'application/json',
        },
      }
    )

    const { id, username, email, role, accessToken, refreshToken: newRefreshToken } = response.data

    // Update tokens
    setAccessToken(accessToken)
    setUser({ id, username, email, role })
    setRefreshTokenCookie(newRefreshToken)

    return accessToken
  } catch (error) {
    // Refresh failed, clear tokens and redirect to login
    clearTokens()
    clearRefreshTokenCookie()
    // Optionally redirect to login page
    window.location.href = '/login'
    throw error
  }
}

// Add request interceptor to include token and check expiry
api.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    let token = getAccessToken()

    // Skip token refresh for auth endpoints
    if (config.url?.includes('/auth/')) {
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    }

    // Check if token is expired
    if (token && isTokenExpired(token)) {
      if (!isRefreshing) {
        isRefreshing = true

        try {
          token = await refreshAccessToken()
          isRefreshing = false
          processQueue(null, token)
        } catch (error) {
          isRefreshing = false
          processQueue(error, null)
          return Promise.reject(error)
        }
      } else {
        // Wait for the ongoing refresh to complete
        return new Promise((resolve, reject) => {
          failedQueue.push({
            resolve: (token: string) => {
              config.headers.Authorization = `Bearer ${token}`
              resolve(config)
            },
            reject
          })
        })
      }
    }

    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Add response interceptor to handle 401 errors
api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean }

    if (error.response?.status === 401 && originalRequest && !originalRequest._retry) {
      originalRequest._retry = true

      try {
        const token = await refreshAccessToken()
        originalRequest.headers.Authorization = `Bearer ${token}`
        return api(originalRequest)
      } catch (refreshError) {
        return Promise.reject(refreshError)
      }
    }

    return Promise.reject(error)
  }
)

export default api
