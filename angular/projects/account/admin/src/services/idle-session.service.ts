
import {
  ApplicationRef,
  ComponentRef,
  createComponent,
  DestroyRef,
  effect,
  EnvironmentInjector,
  inject,
  Injectable,
  signal,
  DOCUMENT
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  merge,
  fromEvent,
  debounceTime,
  distinctUntilChanged,
  interval,
  map,
  startWith,
  switchMap,
  takeWhile,
  tap,
  timer,
  filter,
  Subscription,
  take,
} from 'rxjs';
import { AbpLocalStorageService, AuthService, ConfigStateService } from '@abp/ng.core';

export const IDLE_SESSION_ENABLED_SETTING_KEY = 'Abp.Account.Idle.Enabled';
export const IDLE_TIMEOUT_MINUTES_SETTING_KEY = 'Abp.Account.Idle.IdleTimeoutMinutes';

const MODAL_COUNTDOWN = 60;
const SECONDS = 1000 * 60;

@Injectable({ providedIn: 'root' })
export class IdleSessionService {
  protected readonly configStateService = inject(ConfigStateService);
  protected readonly authService = inject(AuthService);
  protected readonly localStorageService = inject(AbpLocalStorageService);
  protected readonly destroyRef = inject(DestroyRef);
  protected readonly document = inject(DOCUMENT);
  protected readonly environmentInjector = inject(EnvironmentInjector);
  protected readonly applicationRef = inject(ApplicationRef);

  public componentRef: ComponentRef<unknown> | null = null;
  public modalCountdown = signal(MODAL_COUNTDOWN);
  public showModal = signal(false);
  public idleTimeout: Subscription;

  constructor() {
    effect(() => {
      if (!this.modalCountdown() && this.showModal()) {
        this.showModal.set(false);
        this.logout();
      }
    });
  }

  async renderTimeoutModal() {
    if (!this.componentRef) {
      //TODO: Improve this import
      const IdleSessionModalComponent = (await import('../components')).IdleSessionModalComponent;
      this.componentRef = createComponent(IdleSessionModalComponent, {
        environmentInjector: this.environmentInjector,
      });
      this.applicationRef.attachView(this.componentRef.hostView);
    }
  }

  watchUserActivity() {
    merge(
      fromEvent(window, 'mousemove'),
      fromEvent(window, 'keydown'),
      fromEvent(window, 'storage'),
    )
      .pipe(
        filter(() => {
          this.unsubscribeIdleTimeout();
          let isIdleSessionEnabled = this.localStorageService.getItem('isIdleSessionEnabled');
          if (!isIdleSessionEnabled) {
            isIdleSessionEnabled = this.configStateService
              .getSetting(IDLE_SESSION_ENABLED_SETTING_KEY)
              ?.toLowerCase();
          }
          return isIdleSessionEnabled === 'true' && this.authService.isAuthenticated;
        }),
        tap(() => this.startIdleTimeout()),
        debounceTime(500),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe();
  }

  startIdleTimeout(): void {
    const idleTimeoutMinutes = Number(
      this.configStateService.getSetting(IDLE_TIMEOUT_MINUTES_SETTING_KEY),
    );

    if (!idleTimeoutMinutes) {
      return;
    }

    this.idleTimeout = timer(idleTimeoutMinutes * SECONDS)
      .pipe(
        tap(() => {
          this.showModal.set(true);
          this.startCountdown();
        }),
      )
      .subscribe();
  }

  startCountdown(): void {
    let lastCount = this.modalCountdown();

    const visibilityChange$ = fromEvent(document, 'visibilitychange').pipe(
      map(() => !document.hidden),
      startWith(!document.hidden),
      distinctUntilChanged(),
    );

    visibilityChange$
      .pipe(
        filter(isVisible => isVisible && lastCount === MODAL_COUNTDOWN),
        take(1),
        switchMap(() => {
          return interval(1000).pipe(
            map(() => {
              lastCount -= 1;
              return lastCount;
            }),
            takeWhile(count => count >= 0 && this.showModal()),
          );
        }),
      )
      .subscribe(count => {
        this.modalCountdown.set(count);
      });
  }

  staySignedIn() {
    this.showModal.set(false);
    this.modalCountdown.set(MODAL_COUNTDOWN);
    this.unsubscribeIdleTimeout();
  }

  logout() {
    this.unsubscribeIdleTimeout();
    this.authService.logout().subscribe();
  }

  resetCountdown() {
    this.modalCountdown.set(0);
  }

  unsubscribeIdleTimeout() {
    this.idleTimeout?.unsubscribe();
  }
}
