import { AlertService } from './../../../shared/services/alert.service';
import { DialogService } from 'primeng/dynamicdialog';
import { AdminApiRoyaltyApiClient, RoyaltyReportByUserDto } from './../../../api/admin-api.service.generated';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { ConfirmationService } from 'primeng/api';
import { MessageConstants } from 'src/app/shared/constants/message.constants';
@Component({
    selector: 'app-royalty-user',
    templateUrl: './royalty-user.component.html',

})
export class RoyaltyUserComponent implements OnInit, OnDestroy {
    //System variables
    private ngUnsubscribe = new Subject<void>();
    public blockedPanel: boolean = false;
    public items: RoyaltyReportByUserDto[] = [];
    public userName: string = '';
    public fromMonth: number = 1;
    public fromYear: number = new Date().getFullYear();
    public toMonth: number = 12;
    public toYear: number = new Date().getFullYear();

    /**
     *
     */
    constructor(private royaltyService: AdminApiRoyaltyApiClient, public dialogService: DialogService, private alertService: AlertService, private confirmationService: ConfirmationService) {
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.toggleBlockUI(true);
        this.royaltyService.getRoyaltyReportByUser(this.userName, this.fromMonth, this.fromYear, this.toMonth, this.toYear).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (res: RoyaltyReportByUserDto[]) => {
                this.items = res;
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    payForUser(userId: string) {
        this.confirmationService.confirm({
            message: 'Bạn có chắc muốn thanh toán không?',
            accept: () => this.payForUserConfirm(userId)
        })
    }

    payForUserConfirm(userId: string) {
        this.toggleBlockUI(true);
        this.royaltyService.payRoyalty(userId).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: () => {
                this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
                this.loadData();
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    private toggleBlockUI(enabled: boolean) {
        if (enabled == true) {
            this.blockedPanel = true;
        }
        else {
            setTimeout(() => {
                this.blockedPanel = false;
            }, 1000);
        }
    }
}