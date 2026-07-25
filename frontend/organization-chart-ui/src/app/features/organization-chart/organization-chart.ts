import { Component, OnInit, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { OrganizationChartService } from '../../core/services/organization-chart.service';
import { OrganizationChartNode } from '../../core/models/organization-chart';

@Component({
  selector: 'app-organization-chart',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatProgressBarModule],
  templateUrl: './organization-chart.html',
  styleUrl: './organization-chart.scss',
})
export class OrganizationChart implements OnInit {
  roots = signal<OrganizationChartNode[]>([]);
  loading = signal<boolean>(true);
  error = signal<string | null>(null);

  // Department ids whose children are collapsed / whose member list is open.
  private collapsed = signal<Set<number>>(new Set<number>());
  private membersOpen = signal<Set<number>>(new Set<number>());

  totalDepartments = computed(() => this.countNodes(this.roots()));

  constructor(private orgService: OrganizationChartService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.orgService.getChart().subscribe({
      next: data => {
        this.roots.set(data);
        this.loading.set(false);
      },
      error: err => {
        console.error(err);
        this.error.set('Could not load the organization chart.');
        this.loading.set(false);
      }
    });
  }

  // ---- collapse / expand of child departments ----
  isCollapsed(id: number): boolean {
    return this.collapsed().has(id);
  }

  toggleCollapse(id: number): void {
    const next = new Set(this.collapsed());
    if (next.has(id)) {
      next.delete(id);
    } else {
      next.add(id);
    }
    this.collapsed.set(next);
  }

  collapseAll(): void {
    this.collapsed.set(new Set(this.allIds(this.roots())));
  }

  expandAll(): void {
    this.collapsed.set(new Set<number>());
  }

  // ---- show / hide a department's member list ----
  isMembersOpen(id: number): boolean {
    return this.membersOpen().has(id);
  }

  toggleMembers(id: number): void {
    const next = new Set(this.membersOpen());
    if (next.has(id)) {
      next.delete(id);
    } else {
      next.add(id);
    }
    this.membersOpen.set(next);
  }

  // ---- headcount including all sub-departments ----
  totalHeadcount(node: OrganizationChartNode): number {
    return node.users.length +
      node.children.reduce((sum, c) => sum + this.totalHeadcount(c), 0);
  }

  private countNodes(nodes: OrganizationChartNode[]): number {
    return nodes.reduce((sum, n) => sum + 1 + this.countNodes(n.children), 0);
  }

  private allIds(nodes: OrganizationChartNode[]): number[] {
    return nodes.flatMap(n => [n.departmentId, ...this.allIds(n.children)]);
  }
}
