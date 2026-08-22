export const USER_ROLES = ['Admin', 'Professor', 'Aluno'] as const

export type UserRole = (typeof USER_ROLES)[number]
export type CreatableRole = Exclude<UserRole, 'Admin'>

export interface AuthUser {
  id: number
  name: string
  email: string
  role: UserRole
}

export interface User extends AuthUser {
  createdAt: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  token: string
  expiresAt: string
  user: AuthUser
}

export interface CreateUserRequest {
  name: string
  email: string
  password: string
  role: CreatableRole
}

export interface DemoResponse {
  message: string
}
