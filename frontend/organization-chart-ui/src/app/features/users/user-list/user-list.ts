import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../core/services/user.service';
import { User } from '../../../core/models/user';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, MatTableModule,
    MatButtonModule, RouterLink],
  templateUrl: './user-list.html',
  styleUrls: ['./user-list.scss']
})
export class UserList implements OnInit {
  users = signal<User[]>([]);

  displayedColumns: string[] = [
    'employeeNumber',
    'firstName',
    'lastName',
    'title',
    'department',
    'actions'
  ];

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.userService.getAll().subscribe({
      next: data => {
        console.log(data);
        this.users.set(data);
      },
      error: err => console.error(err)
    });
  }
}
