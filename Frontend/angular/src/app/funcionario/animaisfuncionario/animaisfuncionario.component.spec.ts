import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnimaisfuncionarioComponent } from './animaisfuncionario.component';

describe('AnimaisfuncionarioComponent', () => {
  let component: AnimaisfuncionarioComponent;
  let fixture: ComponentFixture<AnimaisfuncionarioComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AnimaisfuncionarioComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AnimaisfuncionarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
