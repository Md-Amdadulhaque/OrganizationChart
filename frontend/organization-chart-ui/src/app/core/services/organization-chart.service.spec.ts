import { TestBed } from '@angular/core/testing';

import { OrganizationChart } from './organization-chart';

describe('OrganizationChart', () => {
  let service: OrganizationChart;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OrganizationChart);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
