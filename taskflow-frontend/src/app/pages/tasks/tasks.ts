import { Component } from '@angular/core';
import { TaskList } from '../../components/task-list/task-list';
import { TaskForm } from '../../components/task-form/task-form';
import { ViewChild } from '@angular/core';

@Component({
  selector: 'app-tasks',
  imports: [TaskList, TaskForm],
  templateUrl: './tasks.html',
  styleUrl: './tasks.css',
})
export class Tasks {
  @ViewChild(TaskList)
  taskListComponent!: TaskList;
  
  onTaskCreated(): void {
    setTimeout(() => this.taskListComponent.loadTasks(), 0);
  }
}
