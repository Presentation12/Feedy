import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AjudagerenteComponent } from './ajudagerente.component';

describe('AjudagerenteComponent', () => {
  let component: AjudagerenteComponent;
  let fixture: ComponentFixture<AjudagerenteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AjudagerenteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AjudagerenteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
