import { AlertService } from './../../../shared/services/alert.service';
import { DialogService } from 'primeng/dynamicdialog';
import { AdminApiRoyaltyApiClient, RoyaltyReportByMonthDto } from './../../../api/admin-api.service.generated';
import { Component, OnDestroy, OnInit } from "@angular/core";
import { Subject, takeUntil } from "rxjs";
import { ConfirmationService } from 'primeng/api';

@Component({
    selector: 'app-royalty-month',
    templateUrl: './royalty-month.component.html'

})
export class RoyaltyMonthComponent implements OnInit, OnDestroy {
    private ngUnsubscribe = new Subject<void>();
    public blockedPanel: boolean = false;
    public items: RoyaltyReportByMonthDto[] = [];
    public userName: string = '';
    public fromMonth: number = 1;
    public fromYear: number = new Date().getFullYear();
    public toMonth: number = 12;
    public toYear: number = new Date().getFullYear();
    constructor(private royaltyService: AdminApiRoyaltyApiClient,
        public dialogService: DialogService,
        private alertService: AlertService,
        private confirmationService: ConfirmationService) {

    }
    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
    ngOnInit(): void {
        this.loadData();
    }

    loadData(){
        this.toggleBlockUI(true);
        this.royaltyService.getRoyaltyReportByMonth(this.userName, this.fromMonth, this.fromYear, this.toMonth, this.toYear).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (res: RoyaltyReportByMonthDto[]) => {
                this.items = res;
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