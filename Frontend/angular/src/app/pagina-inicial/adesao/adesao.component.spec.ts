import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdesaoComponent } from './adesao.component';

describe('AdesaoComponent', () => {
  let component: AdesaoComponent;
  let fixture: ComponentFixture<AdesaoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdesaoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdesaoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
