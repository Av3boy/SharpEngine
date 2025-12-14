import * as React from 'react';
import Box from '@mui/material/Box';
import Collapse from '@mui/material/Collapse';
import IconButton from '@mui/material/IconButton';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';

export type CollapsableGridRowProps = {
  summaryCells: React.ReactNode[]; // cells excluding expand toggle
  details: React.ReactNode; // can be any content, often a nested table
  colSpan: number; // number of columns for details collapse cell
};

export default function CollapsableGridRow(props: CollapsableGridRowProps) {
  const { summaryCells, details, colSpan } = props;
  const [open, setOpen] = React.useState(false);

  return (
    <React.Fragment>
      <TableRow sx={{ '& > *': { borderBottom: 'unset' } }}>
        <TableCell>
          <IconButton
            aria-label="expand row"
            size="small"
            onClick={() => setOpen(!open)}
          >
            {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
          </IconButton>
        </TableCell>
        {summaryCells.map((cell, i) => (
          <TableCell key={i} align={i === 0 ? undefined : 'right'}>
            {cell}
          </TableCell>
        ))}
      </TableRow>
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={colSpan}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <Box sx={{ margin: 1 }}>{details}</Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </React.Fragment>
  );
}
