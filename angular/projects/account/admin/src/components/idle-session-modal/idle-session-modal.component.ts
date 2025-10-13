import { Component, inject } from '@angular/core';
import { LocalizationPipe } from '@abp/ng.core';
import { ModalCloseDirective, ModalComponent } from '@abp/ng.theme.shared';
import { IdleSessionService } from '../../services/idle-session.service';

@Component({
  selector: 'abp-idle-session-modal',
  templateUrl: './idle-session-modal.component.html',
  providers: [IdleSessionService],
  imports: [ModalCloseDirective, LocalizationPipe, ModalComponent],
})
export class IdleSessionModalComponent {
  protected readonly idleSessionService = inject(IdleSessionService);

  showModal = this.idleSessionService.showModal;
  countdown = this.idleSessionService.modalCountdown;

  constructor() {
    this.idleSessionService.watchUserActivity();
  }

  staySignedIn(): void {
    this.idleSessionService.staySignedIn();
  }

  logout(): void {
    this.idleSessionService.resetCountdown();
  }
}
