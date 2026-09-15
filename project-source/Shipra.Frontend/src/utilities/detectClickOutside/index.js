import React, { useEffect, useRef } from 'react';

const DetectClickOutSide = ({ children, open, setOpen }) => {
  const node = useRef();

  const handleClickOutside = e => {
    if (node.current.contains(e.target)) {
      // inside click
      return;
    }
    // outside click
    setOpen(false);
  };

  useEffect(() => {
    if (open) {
      document.addEventListener('mousedown', handleClickOutside);
    } else {
      document.removeEventListener('mousedown', handleClickOutside);
    }
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
      window.removeEventListener('click', handleClickOutside);
    };
  }, [open]);

  return <div ref={node}>{children}</div>;
};

export default DetectClickOutSide;
