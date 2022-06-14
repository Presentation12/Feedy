import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarcarServicoComponent } from './marcar-servico.component';

describe('MarcarServicoComponent', () => {
  let component: MarcarServicoComponent;
  let fixture: ComponentFixture<MarcarServicoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarcarServicoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MarcarServicoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
