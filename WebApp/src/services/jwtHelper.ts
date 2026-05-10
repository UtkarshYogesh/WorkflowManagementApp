// JWT Helper utilities
export const decodeToken = (token: string): any => {
  try {
    const base64Url = token.split('.')[1];
    if (!base64Url) {
  throw new Error('Invalid JWT token: missing payload');
}
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    )
    return JSON.parse(jsonPayload)
  } catch (error) {
    console.error('Error decoding token:', error)
    return null
  }
}

export const isTokenExpired = (token: string | null): boolean => {
  if (!token) return true

  const decoded = decodeToken(token)
  if (!decoded || !decoded.exp) return true

  // exp is in seconds, convert to milliseconds
  const expirationTime = decoded.exp * 1000
  const currentTime = Date.now()

  // Consider token expired if less than 1 minute remaining
  const bufferTime = 60 * 1000 // 1 minute buffer

  return currentTime >= expirationTime - bufferTime
}

export const getTokenExpiryTime = (token: string | null): number | null => {
  if (!token) return null

  const decoded = decodeToken(token)
  if (!decoded || !decoded.exp) return null

  return decoded.exp * 1000 // Convert to milliseconds
}
