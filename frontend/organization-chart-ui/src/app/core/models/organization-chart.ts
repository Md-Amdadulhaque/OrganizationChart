import { User } from './user';

export interface OrganizationChartNode {
  departmentId: number;
  departmentCode: string;
  departmentName: string;
  manager?: User | null;
  users: User[];
  children: OrganizationChartNode[];
}