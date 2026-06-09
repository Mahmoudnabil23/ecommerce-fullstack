// ── Request DTOs ─────────────────────────────────────────────────────────
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
  confirmPassword: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface GoogleAuthRequest {
  idToken: string;
}

// ── Response DTOs ────────────────────────────────────────────────────────
export interface UserSummary {
  id: string;
  fullName: string;
  roles: string[];
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number; // seconds
  user: UserSummary;
}

export interface RegisterResponse {
  userId: string;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
}
