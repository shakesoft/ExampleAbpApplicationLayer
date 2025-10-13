import { Component, OnInit, inject, input, model, signal } from '@angular/core';
import { LocalizationPipe, ShortDateTimePipe } from '@abp/ng.core';
import { ModalCloseDirective, ModalComponent } from '@abp/ng.theme.shared';
import { AccountSessionService, Volo } from '@volo/abp.ng.account/public/proxy';

@Component({
  selector: 'abp-account-session-detail',
  templateUrl: './account-session-detail.component.html',
  imports: [ModalCloseDirective, LocalizationPipe, ShortDateTimePipe, ModalComponent],
})
export class AccountSessionDetailComponent implements OnInit {
  protected readonly service = inject(AccountSessionService);

  modalOptions = { size: 'lg' };

  isModalVisible = model(false);
  sessionId = input.required<string>();
  sessionInfo = signal({} as Volo.Abp.Identity.IdentitySessionDto);

  ngOnInit(): void {
    this.service.get(this.sessionId()).subscribe(this.sessionInfo.set);
  }
}
