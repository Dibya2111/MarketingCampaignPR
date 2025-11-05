import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { DashboardComponent } from './features/dashboard/dashboard';
import { CampaignListComponent } from './features/campaigns/campaign-list/campaign-list';
import { CampaignFormComponent } from './features/campaigns/campaign-form/campaign-form';
import { LeadListComponent } from './features/leads/lead-list/lead-list';
import { LeadFormComponent } from './features/leads/lead-form/lead-form';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'home', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'campaigns', component: CampaignListComponent, canActivate: [authGuard] },
  { path: 'campaigns/new', component: CampaignFormComponent, canActivate: [authGuard] },
  { path: 'campaigns/edit/:id', component: CampaignFormComponent, canActivate: [authGuard] },
  { path: 'leads', component: LeadListComponent, canActivate: [authGuard] },
  { path: 'leads/new', component: LeadFormComponent, canActivate: [authGuard] },
  { path: 'leads/edit/:id', component: LeadFormComponent, canActivate: [authGuard] },
  { path: 'bulk-upload', loadComponent: () => import('./features/bulk-upload/bulk-upload').then(m => m.BulkUploadComponent), canActivate: [authGuard] },
  { path: 'analytics/campaign/:id', loadComponent: () => import('./features/analytics/campaign-dashboard').then(m => m.CampaignDashboardComponent), canActivate: [authGuard] },
  { path: 'analytics/engagement-upload', loadComponent: () => import('./features/analytics/engagement-upload').then(m => m.EngagementUploadComponent), canActivate: [authGuard] },
  { path: 'analytics', loadComponent: () => import('./features/analytics/analytics-dashboard').then(m => m.AnalyticsDashboardComponent), canActivate: [authGuard] },
  { path: 'otp-verification', loadComponent: () => import('./features/auth/otp-verification/otp-verification').then(m => m.OtpVerificationComponent) },

  { path: '**', redirectTo: '/login' }
];


