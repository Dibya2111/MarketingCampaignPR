import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';
import { NavbarComponent } from './shared/components/navbar/navbar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, NavbarComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  title = 'Marketing Campaign Platform';
  showNavbar = false;

  constructor(private router: Router) {}

  ngOnInit(): void {
    // Check initial route on app load
    this.checkNavbarVisibility(this.router.url);
    
    // Listen for route changes
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.checkNavbarVisibility(event.url);
      });
  }

  private checkNavbarVisibility(url: string): void {
    // Remove query parameters and fragments for checking
    const cleanUrl = url.split('?')[0].split('#')[0];
    const authRoutes = ['/login', '/register', '/otp-verification'];
    this.showNavbar = !authRoutes.includes(cleanUrl);
  }
}


