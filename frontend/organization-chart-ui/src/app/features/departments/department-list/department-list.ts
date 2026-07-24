import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DepartmentService } from '../../../core/services/department.service';
import { Department } from '../../../core/models/department';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-department-list',
  standalone: true,
  imports: [CommonModule, MatTableModule,
    MatButtonModule],
  templateUrl: './department-list.html',
  styleUrls: ['./department-list.scss']
})
export class DepartmentList implements OnInit {
  departments: Department[] = [];

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
      next: data => this.departments = data,
      error: err => console.error(err)
    });
  }
}