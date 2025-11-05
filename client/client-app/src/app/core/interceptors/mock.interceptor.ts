import { HttpInterceptorFn } from '@angular/common/http';
import { of } from 'rxjs';

export const mockInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.includes('/auth/login')) {
    return of({
      status: 200,
      body: { token: 'mock-token', user: { id: 1, email: 'test@test.com' } }
    } as any);
  }
  return next(req);
};