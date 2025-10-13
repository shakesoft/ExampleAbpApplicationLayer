import { ChangeDetectorRef, Component, inject } from '@angular/core';
import {
  AbstractControl,
  UntypedFormGroup,
  FormGroupDirective,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import {
  EXTENSIBLE_FORM_VIEW_PROVIDER,
  EXTENSIONS_FORM_PROP,
  EXTENSIONS_FORM_PROP_DATA,
  FormProp,
} from '@abp/ng.components/extensible';
import { AutofocusDirective, ConfigStateService, LocalizationPipe } from '@abp/ng.core';
import {
  ButtonComponent,
  ModalCloseDirective,
  ModalComponent,
  ToasterService,
} from '@abp/ng.theme.shared';
import { ProfileDto } from '@volo/abp.ng.account/public/proxy';
import { AccountService, ManageProfileStateService } from '../../../services';
import { PersonalSettingsVerifyButtonComponent } from '../personal-settings-verify-button/personal-settings-verify-button.component';

@Component({
  selector: 'abp-personal-settings-phone-number',
  templateUrl: './personal-settings-phone-number.component.html',
  viewProviders: [EXTENSIBLE_FORM_VIEW_PROVIDER],
  imports: [
    NgxValidateCoreModule,
    FormsModule,
    ReactiveFormsModule,
    AutofocusDirective,
    ModalCloseDirective,
    AsyncPipe,
    LocalizationPipe,
    PersonalSettingsVerifyButtonComponent,
    ModalComponent,
    ButtonComponent,
  ],
})
export class PersonalSettingsPhoneNumberComponent {
  private formProp = inject<FormProp>(EXTENSIONS_FORM_PROP);
  private propData = inject<ProfileDto>(EXTENSIONS_FORM_PROP_DATA);
  private formGroupDirective = inject(FormGroupDirective);
  private configState = inject(ConfigStateService);
  private accountService = inject(AccountService);
  private toasterService = inject(ToasterService);
  private manageProfileState = inject(ManageProfileStateService);
  private cdr = inject(ChangeDetectorRef);

  public displayName: string;
  public name: string;
  public id: string;
  public isEnablePhoneNumberConfirmation: boolean;
  public initialValue: string;
  public isValueChanged$: Observable<boolean>;
  public isVerified: boolean;
  public modalVisible: boolean;
  public token: string;
  private formGroup: UntypedFormGroup;
  public formControl: AbstractControl;
  modalBusy: boolean;

  constructor() {
    this.name = this.formProp.name;
    this.id = this.formProp.id;
    this.isVerified = this.propData.phoneNumberConfirmed;

    this.displayName = this.formProp.displayName;
    this.formGroup = this.formGroupDirective.control;
    this.formControl = this.formGroup.controls[this.name];
    this.isEnablePhoneNumberConfirmation = this.getIsEnablePhoneNumberConfirmation();
    this.initialValue = this.propData.phoneNumber;
    this.isValueChanged$ = this.formControl.valueChanges.pipe(
      map(value => value !== this.initialValue),
    );
  }

  getIsEnablePhoneNumberConfirmation() {
    return (
      this.configState.getSetting('Abp.Identity.SignIn.EnablePhoneNumberConfirmation') === 'True'
    );
  }

  get userId(): string {
    return this.configState.getDeep('currentUser.id');
  }

  initPhoneNumberConfirmation = () => {
    if (this.formControl.invalid) {
      return;
    }
    const phoneNumber = this.formControl.value;
    const userId = this.userId;
    this.accountService
      .sendPhoneNumberConfirmationToken({
        phoneNumber,
        userId,
      })
      .pipe(tap(() => (this.token = '')))
      .subscribe(this.openModal);
  };

  openModal = () => {
    this.modalVisible = true;
    this.cdr.detectChanges();
  };

  removeModal = () => {
    this.modalVisible = false;
  };

  setPhoneNumberAsConfirmed = () => {
    const profile = { ...this.manageProfileState.getProfile(), phoneNumberConfirmed: true };
    this.manageProfileState.setProfile(profile);
  };

  confirmPhoneNumber() {
    this.accountService
      .confirmPhoneNumber({ token: this.token, userId: this.userId })
      .pipe(tap(this.setPhoneNumberAsConfirmed), tap(this.removeModal))
      .subscribe(() => {
        this.toasterService.success('AbpAccount::Verified', '', { life: 5000 });
      });
  }
}
