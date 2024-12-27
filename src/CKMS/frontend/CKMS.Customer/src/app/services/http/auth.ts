import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { SessionService } from '../session/session.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private sessionService: SessionService, private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Define the URLs that should bypass the JWT token
    const bypassUrls = ['/login', '/register'];

    // Check if the request URL matches any bypass URLs
    const shouldBypass = bypassUrls.some(url => req.url.includes(url));
    console.log("Auth " + req.url)
    if (shouldBypass) {
      // Proceed without adding the token
      return next.handle(req);
    }

    // Get the token from AuthService
    const token = this.sessionService.getToken();

    console.log("Interceptor : " + token)

    if (token && !this.sessionService.isTokenExpired()) {
      // Clone the request and add the Authorization header with the JWT token
      const cloned = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
      });
      return next.handle(cloned);
    }

    // If no token is found or it is expired, proceed with the original request
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 || error.status === 403) {
          // Token is invalid or expired
          this.sessionService.deleteDetails();
          this.router.navigate(['/login']);
        }
        return throwError(error);
      })
    );
  }
}