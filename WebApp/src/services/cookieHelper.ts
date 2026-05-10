// Cookie management utilities
export const setRefreshTokenCookie = (token: string, expiryDays: number = 7) => {
  const date = new Date()
  date.setTime(date.getTime() + expiryDays * 24 * 60 * 60 * 1000)
  const expires = 'expires=' + date.toUTCString()
  document.cookie = `refreshToken=${token};${expires};path=/;SameSite=Strict`
}

export const getRefreshTokenCookie = (): string | null => {
  const name = 'refreshToken='
  const decodedCookie = decodeURIComponent(document.cookie)
  const cookieArray = decodedCookie.split(';')

  for (let cookie of cookieArray) {
    cookie = cookie.trim()
    if (cookie.indexOf(name) === 0) {
      return cookie.substring(name.length)
    }
  }
  return null
}

export const clearRefreshTokenCookie = () => {
  document.cookie = 'refreshToken=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;'
}
