import { AlertService } from './../../../shared/services/alert.service';
import { DialogService } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiSeriesApiClient, SeriesDto, SeriesInListDto, SeriesInListDtoPagedResult } from './../../../api/admin-api.service.generated';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { SeriesDetailComponent } from './series-detail.component';
import { MessageConstants } from 'src/app/shared/constants/message.constants';
import { SeriesPostsComponent } from './series-posts.component';
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

    showAddModal() {
        const ref = this.dialogService.open(SeriesDetailComponent, {
            header: 'Thêm mới series bài viết',
            width: '70%'
        });
        ref.onClose.subscribe((data: SeriesDto) => {
            if (data) {
                this.alertService.showSuccess(MessageConstants.CREATED_OK_MSG);
                this.selectedItems = [];
                this.loadData();
            }
        });
    }
    showEditModal() {
        if (this.selectedItems.length == 0) {
            this.alertService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
            return;
        }
        var id = this.selectedItems[0].id;
        const ref = this.dialogService.open(SeriesDetailComponent, {
            data: { id: id },
            header: 'Cập nhật series bài viết',
            width: '70%',
        });
        ref.onClose.subscribe((data: SeriesDto) => {
            if (data) {
                this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
                this.selectedItems = [];
                this.loadData();
            }
        })
    }
    showPosts() { 
        if (this.selectedItems.length == 0) {
            this.alertService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
            return;
        }
        var id = this.selectedItems[0].id;
        const ref = this.dialogService.open(SeriesPostsComponent, {
            data: { id: id },
            header: 'Quản lý danh sách bài viết',
            width: '70%',
        });
        ref.onClose.subscribe((data: SeriesDto) => {
            if (data) {
                this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
                this.selectedItems = [];
                this.loadData();
            }
        })
    }
    deleteItems() {
        if (this.selectedItems.length == 0) {
            this.alertService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
            return;
        }
        var ids = [];
        this.selectedItems.forEach(el => ids.push(el.id));
        this.confirmationService.confirm({
            message: MessageConstants.CONFIRM_DELETE_MSG,
            accept: () => this.deleteItemsConfirm(ids)
        })
    }
    deleteItemsConfirm(ids: any[]) { 
        this.toggleBlockUI(true);
        this.seriesService.deleteSeries(ids).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: () => {
                this.alertService.showSuccess(MessageConstants.DELETED_OK_MSG);
                this.selectedItems = [];
                this.loadData();
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    pageChanged(event: any): void {
        this.pageIndex = event.page + 1;
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