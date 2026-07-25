import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DepartmentService } from '../../../core/services/department.service';
import { Department } from '../../../core/models/department';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-department-list',
  standalone: true,
  imports: [CommonModule, MatTableModule,
    MatButtonModule,RouterLink],
  templateUrl: './department-list.html',
  styleUrls: ['./department-list.scss']
})
export class DepartmentList implements OnInit {
  departments = signal<Department[]>([]);

 displayedColumns: string[] = [
  'code',
  'name',
  'parent',
  'actions'
];
  constructor(private departmentService: DepartmentService) { }

  ngOnInit(): void {
    this.loadDepartments();
  }

  private loadDepartments(): void {
  this.departmentService.getAll().subscribe({
    next: data => {
      console.log(data);
      this.departments.set(data);
    },
    error: err => console.error(err)
  });
}

  deleteDepartment(id: number): void {
    if (!confirm('Are you sure you want to delete this department?'))
      return;

    this.departmentService.delete(id).subscribe({
      next: () => this.loadDepartments(),
      error: err => console.error(err)
    });
  }
}