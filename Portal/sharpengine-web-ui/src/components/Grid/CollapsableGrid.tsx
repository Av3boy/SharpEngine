import * as React from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import TableFooter from '@mui/material/TableFooter';
import TablePagination from '@mui/material/TablePagination';
import CollapsableGridRow from './CollapsableGridRow';
import CollapsableGridFooter, { TablePaginationActions } from './CollapsableGridFooter';

export type CollapsableGridProps<T> = {
  items: T[];
  renderHeaders: React.ReactNode[]; // cells excluding expand toggle column
  keyExtractor: (item: T, index: number) => string;
  renderSummaryCells: (item: T, index: number) => React.ReactNode[]; // summary row cells
  renderDetails: (item: T, index: number) => React.ReactNode; // content inside collapse
  rowsPerPageOptions?: Array<number | { label: string; value: number }>;
  initialRowsPerPage?: number;
  enablePagination?: boolean;
};

export default function CollapsableGrid<T>(props: CollapsableGridProps<T>) {
  const {
    items,
    renderHeaders,
    keyExtractor,
    renderSummaryCells,
    renderDetails,
    rowsPerPageOptions = [5, 10, 25, { label: 'All', value: -1 }],
    initialRowsPerPage = 5,
    enablePagination = true,
  } = props;

  const [rowsPerPage, setRowsPerPage] = React.useState(initialRowsPerPage);
  const [page, setPage] = React.useState(0);

  const data = React.useMemo(() => {
    if (!enablePagination || rowsPerPage === -1)
      return items;

    const start = page * rowsPerPage;
    return items.slice(start, start + rowsPerPage);
  }, [items, page, rowsPerPage, enablePagination]);

  const handleChangePage = (
    event: React.MouseEvent<HTMLButtonElement> | null,
    newPage: number,
  ) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    const next = parseInt(event.target.value, 10);
    setRowsPerPage(next);
    setPage(0);
  };

  return (
    <TableContainer component={Paper}>
      <Table aria-label="collapsible table">
        <TableHead>
          <TableRow>
            <TableCell />
            {renderHeaders.map((h, i) => (
              <TableCell key={i} align={i === 0 ? undefined : 'right'}>
                {h}
              </TableCell>
            ))}
          </TableRow>
        </TableHead>
        <TableBody>
          {data.map((item, index) => (
            <CollapsableGridRow
              key={keyExtractor(item, index)}
              summaryCells={renderSummaryCells(item, index)}
              details={renderDetails(item, index)}
              colSpan={renderHeaders.length + 1}
            />
          ))}
        </TableBody>
        {enablePagination && (
          <TableFooter>
            <TableRow>
              <TablePagination
                rowsPerPageOptions={rowsPerPageOptions}
                colSpan={renderHeaders.length}
                count={items.length}
                rowsPerPage={rowsPerPage}
                page={page}
                slotProps={{
                  select: {
                    inputProps: {
                      'aria-label': 'rows per page',
                    },
                    native: true,
                  },
                }}
                onPageChange={handleChangePage}
                onRowsPerPageChange={handleChangeRowsPerPage}
                ActionsComponent={TablePaginationActions}
              />
            </TableRow>
          </TableFooter>
        )}
      </Table>
      <CollapsableGridFooter />
    </TableContainer>
  );
}