export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  token?: string;
  username?: string;
  role?: string;
  userId?: number;
  requiresOtp?: boolean;
  otpMessage?: string;
}

export interface User {
  username: string;
  role: string;
  token: string;
}

