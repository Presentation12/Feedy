import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ServicosgerenteComponent } from './servicosgerente.component';

describe('ServicosgerenteComponent', () => {
  let component: ServicosgerenteComponent;
  let fixture: ComponentFixture<ServicosgerenteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ServicosgerenteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ServicosgerenteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
