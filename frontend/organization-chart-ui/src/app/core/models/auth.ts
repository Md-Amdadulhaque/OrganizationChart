export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  role: string;
  expiresAt: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  role: string;
}
