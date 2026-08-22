import { createContext } from 'react'
import type { ReactNode } from 'react'
import type { AuthUser, LoginRequest, UserRole } from '../types/auth'

export interface AuthContextValue {
  user: AuthUser | null
  token: string | null
  isAuthenticated: boolean
  isLoading: boolean
  role: UserRole | null
  login: (credentials: LoginRequest) => Promise<AuthUser>
  logout: () => void
}

export interface AuthProviderProps {
  children: ReactNode
}

export const AuthContext = createContext<AuthContextValue | undefined>(undefined)
