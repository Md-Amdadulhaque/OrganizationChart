import { TestBed } from '@angular/core/testing';

import { Import } from './import';

describe('Import', () => {
  let service: Import;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Import);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
