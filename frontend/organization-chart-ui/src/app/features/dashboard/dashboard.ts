import { Component, OnInit, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { DepartmentService } from '../../core/services/department.service';
import { UserService } from '../../core/services/user.service';
import { Department } from '../../core/models/department';
import { User } from '../../core/models/user';

interface Breakdown {
  label: string;
  count: number;
  percent: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatProgressBarModule
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  departments = signal<Department[]>([]);
  users = signal<User[]>([]);
  loading = signal<boolean>(true);
  error = signal<string | null>(null);

  totalDepartments = computed(() => this.departments().length);
  totalUsers = computed(() => this.users().length);

  topLevelDepartments = computed(
    () => this.departments().filter(d => d.parentDepartmentId == null).length
  );

  avgUsersPerDepartment = computed(() => {
    const depts = this.totalDepartments();
    if (depts === 0) return 0;
    return Math.round((this.totalUsers() / depts) * 10) / 10;
  });

  // Users grouped by department, largest first.
  usersPerDepartment = computed<Breakdown[]>(() => {
    const counts = new Map<string, number>();
    for (const u of this.users()) {
      const key = u.departmentName?.trim() || 'Unassigned';
      counts.set(key, (counts.get(key) ?? 0) + 1);
    }
    return this.toBreakdown(counts);
  });

  // Users grouped by job title, largest first (top 8).
  usersByTitle = computed<Breakdown[]>(() => {
    const counts = new Map<string, number>();
    for (const u of this.users()) {
      const key = u.title?.trim() || 'No title';
      counts.set(key, (counts.get(key) ?? 0) + 1);
    }
    return this.toBreakdown(counts).slice(0, 8);
  });

  constructor(
    private departmentService: DepartmentService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      departments: this.departmentService.getAll(),
      users: this.userService.getAll()
    }).subscribe({
      next: ({ departments, users }) => {
        this.departments.set(departments);
        this.users.set(users);
        this.loading.set(false);
      },
      error: err => {
        console.error(err);
        this.error.set('Could not load dashboard data.');
        this.loading.set(false);
      }
    });
  }

  private toBreakdown(counts: Map<string, number>): Breakdown[] {
    const max = Math.max(0, ...counts.values());
    return Array.from(counts.entries())
      .map(([label, count]) => ({
        label,
        count,
        percent: max === 0 ? 0 : Math.round((count / max) * 100)
      }))
      .sort((a, b) => b.count - a.count);
  }
}
