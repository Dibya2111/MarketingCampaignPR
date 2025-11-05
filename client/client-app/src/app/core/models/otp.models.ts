export interface GenerateOtpRequest {
  userId: number;
}

export interface VerifyOtpRequest {
  userId: number;
  otpCode: string;
}

export interface OtpResponse {
  success: boolean;
  message: string;
  token?: string;
  remainingAttempts: number;
}

