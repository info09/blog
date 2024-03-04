import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../../../app/shared/auth.guard';
import { TransactionComponent } from './transactions/transactions.component';
const routes: Routes = [
  {
    path: '',
    redirectTo: 'transactions',
    pathMatch: 'full',
  },
//   {
//     path: 'royalty-month',
//     component: RoyaltyMonthComponent,
//     data: {
//       title: 'Thống kê tháng',
//       requiredPolicy: 'Permissions.Royalty.View',
//     },
//     canActivate: [AuthGuard],
//   },
//   {
//     path: 'royalty-user',
//     component: RoyaltyUserComponent,
//     data: {
//       title: 'Thống kê tác giả',
//       requiredPolicy: 'Permissions.Royalty.View',
//     },
//     canActivate: [AuthGuard],
//   },
  {
    path: 'transactions',
    component: TransactionComponent,
    data: {
      title: 'Giao dịch',
      requiredPolicy: 'Permissions.Royalty.View',
    },
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RoyaltyRoutingModule {}