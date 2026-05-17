import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../services/task';
import { OnInit } from '@angular/core';
import { Task } from '../../models/task';
import { Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  standalone: true,
  selector: 'app-task-list',
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatSnackBarModule,
  ],
  templateUrl: './task-list.html',
  styleUrl: './task-list.css',
})
export class TaskList implements OnInit {
  @Output()
  taskDeleted = new EventEmitter<void>();

  editingTaskId: number | null = null;

  tasks: Task[] = [];

  isLoading = false;

  errorMessage = '';

  searchTerm = '';

  currentPage = 1;

  pageSize = 5;

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredTasks().length / this.pageSize));
  }

  private clampCurrentPage(): void {
    if (this.currentPage > this.totalPages) {
      this.currentPage = this.totalPages;
    }

    if (this.currentPage < 1) {
      this.currentPage = 1;
    }
  }

  constructor(
    private taskService: TaskService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  private formatDueDateAsUtc(dueDate: string | null): string | null {
    if (!dueDate) {
      return null;
    }

    if (/^\d{4}-\d{2}-\d{2}$/.test(dueDate)) {
      return new Date(`${dueDate}T00:00:00Z`).toISOString();
    }

    return dueDate;
  }

  loadTasks(): void {
    this.isLoading = true;

    this.taskService.getTasks().subscribe({
      next: (response: Task[]) => {
        this.tasks = response;
        this.clampCurrentPage();
        this.isLoading = false;
        this.cdr.detectChanges();
      },

      error: () => {
        this.errorMessage = 'Failed to load tasks';
        this.clampCurrentPage();
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  deleteTask(id: number): void {
    this.taskService.deleteTask(id).subscribe({
      next: () => {
        this.snackBar.open('Task deleted', 'Close', {
          duration: 3000,
        });
        setTimeout(() => {
          this.errorMessage = '';
          this.taskDeleted.emit();
          this.cdr.detectChanges();
        }, 0);
      },

      error: () => {
        this.snackBar.open('Failed to delete task', 'Close', {
          duration: 3000,
        });
      },
    });
  }

  startEdit(id: number): void {
    this.editingTaskId = id;
  }

  saveEdit(task: Task, newTitle: string): void {
    const updatedTask = {
      title: newTitle,
      description: task.description,
      isCompleted: task.isCompleted,
      dueDate: this.formatDueDateAsUtc(task.dueDate),
      categoryId: task.category?.id ?? null,
    };

    this.taskService.updateTask(task.id, updatedTask).subscribe({
      next: () => {
        this.snackBar.open('Task updated', 'Close', {
          duration: 3000,
        });
        this.errorMessage = '';
        setTimeout(() => {
          this.editingTaskId = null;
          this.loadTasks();
          this.cdr.detectChanges();
        }, 0);
      },

      error: () => {
        this.snackBar.open('Failed to update task', 'Close', {
          duration: 3000,
        });
        setTimeout(() => {
          this.errorMessage = 'Failed to update task';
          this.cdr.detectChanges();
        }, 0);
      },
    });
  }

  filteredTasks(): Task[] {
    return this.tasks.filter((task) =>
      task.title.toLowerCase().includes(this.searchTerm.toLowerCase()),
    );
  }

  paginatedTasks(): Task[] {
    const start = (this.currentPage - 1) * this.pageSize;

    const end = start + this.pageSize;

    return this.filteredTasks().slice(start, end);
  }

  nextPage(): void {
    const totalPages = Math.ceil(this.filteredTasks().length / this.pageSize);

    if (this.currentPage < totalPages) {
      this.currentPage++;
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }
}
