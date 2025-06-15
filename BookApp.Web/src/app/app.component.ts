import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { ThemeToggleComponent } from './shared/components/theme-toggle/theme-toggle.component';
import { FaviconComponent } from './shared/components/favicon/favicon.component';
import './fontawesome.config'; // Import FontAwesome configuration

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, FooterComponent, ThemeToggleComponent, FaviconComponent],
  template: `
    <div class="app-root">
      <app-favicon></app-favicon>
      <app-navbar></app-navbar>
      <section class="main-content container mt-4" role="main">
        <router-outlet></router-outlet>
      </section>
      <app-footer></app-footer>
      <app-theme-toggle></app-theme-toggle>
    </div>
  `,
  styles: [`
    .app-root {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
    .main-content {
      flex: 1;
    }
  `]
})
export class AppComponent {
  title = 'BookApp';
}