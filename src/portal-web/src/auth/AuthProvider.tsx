import { useEffect, useMemo, useState } from 'react'
import { api, AUTH_TOKEN_KEY } from '../api/client'
import type { AuthUser, LoginRequest, LoginResponse } from '../types/auth'
import {
  AuthContext,
  type AuthContextValue,
  type AuthProviderProps,
} from './AuthContext'

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<AuthUser | null>(null)
  const [token, setToken] = useState<string | null>(() =>
    localStorage.getItem(AUTH_TOKEN_KEY),
  )
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const storedToken = localStorage.getItem(AUTH_TOKEN_KEY)

    if (!storedToken) {
      setIsLoading(false)
      return
    }

    let isActive = true

    api
      .get<AuthUser>('/api/auth/me')
      .then(({ data }) => {
        if (isActive) {
          setToken(storedToken)
          setUser(data)
        }
      })
      .catch(() => {
        localStorage.removeItem(AUTH_TOKEN_KEY)

        if (isActive) {
          setToken(null)
          setUser(null)
        }
      })
      .finally(() => {
        if (isActive) {
          setIsLoading(false)
        }
      })

    return () => {
      isActive = false
    }
  }, [])

  async function login(credentials: LoginRequest): Promise<AuthUser> {
    const { data } = await api.post<LoginResponse>('/api/auth/login', credentials)

    localStorage.setItem(AUTH_TOKEN_KEY, data.token)
    setToken(data.token)
    setUser(data.user)

    return data.user
  }

  function logout() {
    localStorage.removeItem(AUTH_TOKEN_KEY)
    setToken(null)
    setUser(null)
  }

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      token,
      isAuthenticated: Boolean(token && user),
      isLoading,
      role: user?.role ?? null,
      login,
      logout,
    }),
    [isLoading, token, user],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
