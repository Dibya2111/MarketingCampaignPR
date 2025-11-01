import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { CampaignListComponent } from './features/campaigns/campaign-list/campaign-list.component';
import { CampaignFormComponent } from './features/campaigns/campaign-form/campaign-form.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'home', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'campaigns', component: CampaignListComponent, canActivate: [authGuard] },
  { path: 'campaigns/new', component: CampaignFormComponent, canActivate: [authGuard] },
  { path: 'campaigns/edit/:id', component: CampaignFormComponent, canActivate: [authGuard] },
  { path: 'leads', loadComponent: () => import('./features/leads/lead-list/lead-list.component').then(m => m.LeadListComponent), canActivate: [authGuard] },
  { path: 'leads/new', loadComponent: () => import('./features/leads/lead-form/lead-form.component').then(m => m.LeadFormComponent), canActivate: [authGuard] },
  { path: 'leads/edit/:id', loadComponent: () => import('./features/leads/lead-form/lead-form.component').then(m => m.LeadFormComponent), canActivate: [authGuard] },
  { path: '**', redirectTo: '/login' }
];
