import axios from 'axios'

export const AUTH_TOKEN_KEY = 'prometheus.auth.token'

const apiUrl = import.meta.env.VITE_API_URL?.trim() || 'http://localhost:5000'

export const api = axios.create({
  baseURL: apiUrl,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10_000,
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem(AUTH_TOKEN_KEY)

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})
