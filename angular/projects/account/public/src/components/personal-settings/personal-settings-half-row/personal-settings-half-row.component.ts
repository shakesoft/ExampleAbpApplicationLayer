import { Component, inject } from '@angular/core';
import { UntypedFormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { LocalizationPipe } from '@abp/ng.core';
import {
  EXTENSIBLE_FORM_VIEW_PROVIDER,
  FormProp,
  EXTENSIONS_FORM_PROP,
} from '@abp/ng.components/extensible';

@Component({
  selector: 'abp-personal-settings-half-row',
  template: ` <div class="w-50 d-inline">
    <div class="mb-3 ">
      <label [attr.for]="name" class="form-label">{{ displayName | abpLocalization }} </label>
      <input
        type="text"
        [attr.id]="id"
        class="form-control"
        [attr.name]="name"
        [formControlName]="name"
      />
    </div>
  </div>`,
  styles: [],
  viewProviders: [EXTENSIBLE_FORM_VIEW_PROVIDER],
  imports: [FormsModule, ReactiveFormsModule, NgxValidateCoreModule, LocalizationPipe],
})
export class PersonalSettingsHalfRowComponent {
  private formProp = inject<FormProp>(EXTENSIONS_FORM_PROP);
  public displayName: string;
  public name: string;
  public id: string;
  public formGroup: UntypedFormGroup;

  constructor() {
    this.displayName = this.formProp.displayName;
    this.name = this.formProp.name;
    this.id = this.formProp.id;
  }
}
