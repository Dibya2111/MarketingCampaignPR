import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { OtpService } from '../../../core/services/otp.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-otp-verification',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './otp-verification.html',
  styleUrls: ['./otp-verification.css']
})
export class OtpVerificationComponent implements OnInit {
  otpForm: FormGroup;
  loading = false;
  error = '';
  success = '';
  userId = 0;
  remainingAttempts = 3;
  countdown = 60;
  canResend = false;

  constructor(
    private fb: FormBuilder,
    private otpService: OtpService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.otpForm = this.fb.group({
      otpCode: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });
  }

  ngOnInit(): void {
    this.userId = Number(this.route.snapshot.queryParams['userId']);
    if (!this.userId) {
      this.router.navigate(['/login']);
      return;
    }
    this.startCountdown();
  }

  startCountdown(): void {
    this.countdown = 60;
    this.canResend = false;
    const timer = setInterval(() => {
      this.countdown--;
      if (this.countdown <= 0) {
        this.canResend = true;
        clearInterval(timer);
      }
    }, 1000);
  }

  onSubmit(): void {
    if (this.otpForm.valid) {
      this.loading = true;
      this.error = '';

      const request = {
        userId: this.userId,
        otpCode: this.otpForm.value.otpCode
      };

      this.otpService.verifyOtp(request).subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success && response.token) {
            localStorage.setItem('token', response.token);
            this.authService.setCurrentUser();
            this.router.navigate(['/dashboard']);
          } else {
            this.error = response.message || 'OTP verification failed';
          }
        },
        error: (err) => {
          this.error = err.error?.message || 'OTP verification failed';
          this.remainingAttempts = err.error?.remainingAttempts || 0;
          this.loading = false;
          
          if (this.remainingAttempts <= 0) {
            setTimeout(() => this.router.navigate(['/login']), 2000);
          }
        }
      });
    }
  }

  resendOtp(): void {
    if (!this.canResend) return;

    this.loading = true;
    this.otpService.generateOtp({ userId: this.userId }).subscribe({
      next: (response) => {
        this.success = response.message;
        this.remainingAttempts = 3;
        this.startCountdown();
        this.loading = false;
        this.otpForm.reset();
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to resend OTP';
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/login']);
  }
}

