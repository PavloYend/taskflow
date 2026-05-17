import { Component, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../services/task';
import { CreateTask } from '../../models/create-task';
import { Output, EventEmitter } from '@angular/core';
import { CategoryService } from '../../services/category';
import { Category } from '../../models/category';
import { OnInit } from '@angular/core';
import { AuthService } from '../../services/auth';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  standalone: true,
  selector: 'app-task-form',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatSnackBarModule
  ],
  templateUrl: './task-form.html',
  styleUrl: './task-form.css',
})
export class TaskForm implements OnInit {
  @Output()
  taskCreated = new EventEmitter<void>();

  categories: Category[] = [];
  categoriesLoaded = false;

  taskForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private categoryService: CategoryService,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
  ) {
    this.taskForm = this.fb.group({
      title: ['', [Validators.required]],
      description: [''],
      dueDate: [''],
      categoryId: [null],
    });
  }

  private formatDueDateAsUtc(dueDate: string): string {
    return new Date(`${dueDate}T00:00:00Z`).toISOString();
  }

  onSubmit(): void {
    if (this.taskForm.invalid) {
      return;
    }

    const raw = this.taskForm.value;
    const taskData: CreateTask = {
      title: raw.title,
      description: raw.description || '',
    };

    if (raw.dueDate?.trim()) {
      taskData.dueDate = this.formatDueDateAsUtc(raw.dueDate);
    }

    if (raw.categoryId != null) {
      taskData.categoryId = raw.categoryId;
    }

    this.taskService.createTask(taskData).subscribe({
      next: () => {
        this.snackBar.open('Task created', 'Close', {
          duration: 3000,
        });
        this.taskCreated.emit();
        this.taskForm.reset();
      },

      error: (err) => {
        let errorMessage = 'Failed to create task';
        if (err?.error?.message) {
          errorMessage = err.error.message;
        }
        this.snackBar.open(errorMessage, 'Close', {
          duration: 3000,
        });
      },
    });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    if (!this.authService.isAuthenticated()) {
      return;
    }

    this.categoryService.getCategories().subscribe({
      next: (response) => {
        this.categories = response;
        this.categoriesLoaded = true;
        this.cdr.detectChanges();
      },

      error: () => {
      },
    });
  }
}
