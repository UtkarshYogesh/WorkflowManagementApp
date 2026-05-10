// In-memory token storage
interface TokenStore {
  accessToken: string | null
  user: any | null
}

const tokenStore: TokenStore = {
  accessToken: null,
  user: null
}

export const setAccessToken = (token: string) => {
  tokenStore.accessToken = token
}

export const getAccessToken = () => {
  return tokenStore.accessToken
}

export const setUser = (user: any) => {
  tokenStore.user = user
}

export const getUser = () => {
  return tokenStore.user
}

export const clearTokens = () => {
  tokenStore.accessToken = null
  tokenStore.user = null
}
