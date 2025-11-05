import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, interval, Subscription } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class SessionTimeoutService {
  private timeoutDuration = 30 * 60 * 1000;//used for timeout
  private timeoutSubscription?: Subscription;
  private countdownSubscription?: Subscription;
  
  private remainingTimeSubject = new BehaviorSubject<string>('30:00');
  public remainingTime$ = this.remainingTimeSubject.asObservable();

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  startSession(): void {
    this.resetTimer();
  }

  resetTimer(): void {
    this.clearTimers();
    
    const startTime = Date.now();
    const endTime = startTime + this.timeoutDuration;
    
    // Countdown updated Function
    this.countdownSubscription = interval(1000).subscribe(() => {
      const now = Date.now();
      const remaining = Math.max(0, endTime - now);
      
      const minutes = Math.floor(remaining / 60000);
      const seconds = Math.floor((remaining % 60000) / 1000);
      const timeString = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
      
      this.remainingTimeSubject.next(timeString);
      if (remaining <= 0) {
        this.handleSessionExpiry();
      }
    });
  }

  private handleSessionExpiry(): void {
    this.clearTimers();
    this.authService.logout();
    this.router.navigate(['/login'], { 
      state: { message: 'Session Timed Out - Unauthorized Access' }
    });
  }

  private clearTimers(): void {
    if (this.timeoutSubscription) {
      this.timeoutSubscription.unsubscribe();
    }
    if (this.countdownSubscription) {
      this.countdownSubscription.unsubscribe();
    }
  }

  endSession(): void {
    this.clearTimers();
    this.remainingTimeSubject.next('00:00');
  }
}

