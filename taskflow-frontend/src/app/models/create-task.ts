export interface CreateTask {
  title: string;
  description: string;
  dueDate?: string | null;
  categoryId?: number | null;
}