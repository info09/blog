import { AlertService } from './../../../shared/services/alert.service';
import { DialogService } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiSeriesApiClient, SeriesInListDto, SeriesInListDtoPagedResult } from './../../../api/admin-api.service.generated';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
@Component({
    selector: 'app-series',
    templateUrl: './series.component.html'
})
export class SeriesComponent implements OnInit, OnDestroy {
    //System variables
    private ngUnsubscribe = new Subject<void>();
    public blockedPanel: boolean = false;

    //Paging variables
    public pageIndex: number = 1;
    public pageSize: number = 10;
    public totalCount: number | undefined;

    //Business variables
    public items: SeriesInListDto[];
    public selectedItems: SeriesInListDto[] = [];
    public keyword: string = '';
    constructor(private seriesService: AdminApiSeriesApiClient, public dialogService: DialogService, private alertService: AlertService, private confirmationService: ConfirmationService) { }
    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
    ngOnInit(): void {
        this.loadData();
    }

    loadData(selectionId = null) {
        this.toggleBlockUI(true);
        this.seriesService.getAllSeriesPaging(this.keyword, this.pageIndex, this.pageSize).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (res: SeriesInListDtoPagedResult) => {
                this.items = res.results || [];
                this.totalCount = res.rowCount;
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    showAddModal() { }
    showEditModal() { }
    showPosts() { }
    deleteItems() { }
    deleteItemsConfirm(ids: any[]) { }

    pageChanged(event: any): void {
        this.pageIndex = event.page;
        this.pageSize = event.rows;
        this.loadData();
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