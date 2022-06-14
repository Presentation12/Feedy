import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddanimalComponent } from './addanimal.component';

describe('AddanimalComponent', () => {
  let component: AddanimalComponent;
  let fixture: ComponentFixture<AddanimalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddanimalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddanimalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
