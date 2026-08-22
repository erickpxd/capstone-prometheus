import axios from 'axios'

interface ApiErrorBody {
  message?: string
  title?: string
  errors?: Record<string, string[]>
}

export function getApiErrorMessage(
  error: unknown,
  fallback = 'Não foi possível concluir a operação.',
): string {
  if (!axios.isAxiosError<ApiErrorBody>(error)) {
    return fallback
  }

  if (!error.response) {
    return 'Não foi possível conectar à API. Verifique se o backend está em execução.'
  }

  const body = error.response.data

  if (body?.message) {
    return body.message
  }

  const validationMessages = body?.errors
    ? Object.values(body.errors).flat().filter(Boolean)
    : []

  if (validationMessages.length > 0) {
    return validationMessages.join(' ')
  }

  if (body?.title) {
    return body.title
  }

  if (error.response.status === 401) {
    return 'Credenciais inválidas ou sessão expirada.'
  }

  if (error.response.status === 403) {
    return 'Seu perfil não possui permissão para realizar esta operação.'
  }

  return fallback
}
