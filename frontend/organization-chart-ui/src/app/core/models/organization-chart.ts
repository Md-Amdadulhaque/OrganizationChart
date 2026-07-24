import { User } from './user';

export interface OrganizationChartNode {
  departmentId: number;
  departmentCode: string;
  departmentName: string;
  users: User[];
  children: OrganizationChartNode[];
}