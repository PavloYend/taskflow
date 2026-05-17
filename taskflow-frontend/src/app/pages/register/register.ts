import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { OnInit } from '@angular/core';
import { AuthService } from '../../services/auth';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  standalone: true,
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSnackBarModule,
  ],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register implements OnInit {
  registerForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar,
  ) {}

  isLoading = false;

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading = true;
    
    this.authService.register(this.registerForm.value).subscribe({
      next: (response: string) => {
        this.isLoading = false;
        this.authService.saveToken(response);
        this.snackBar.open('Registration successful', 'Close', {
          duration: 3000,
        });
        this.router.navigate(['/tasks']);
      },

      error: (err) => {
        this.isLoading = false;
        let errorMessage = 'Registration failed';
        if (typeof err?.error === 'string') {
          errorMessage = err.error;
        } else {
          errorMessage = err?.error?.message || err?.error?.errors?.[0] || 'Registration failed';
        }
        this.snackBar.open(errorMessage, 'Close', {
          duration: 5000,
        });
      },
    });
  }
}
