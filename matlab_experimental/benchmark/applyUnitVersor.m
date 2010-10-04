function t = applyUnitVersor(rbm, pt)
% function t = applyUnitVersor(rbm, pt)
%
% Benchmark code to test 'apply unit versor' in native matlab
% rbm must be array of length 8
% pt must be array of length 4

% rbm = [1 2 3 4 5 6 7 8]
% pt = [1 2 3 4]
% tic,for j=1:100000, x = applyUnitVersor(rbm, pt); end; toc,


t = zeros(1, 4);
t(1) = -rbm(2)*rbm(2)*pt(1)+2*rbm(2)*rbm(3)*pt(3)+-2*rbm(2)*rbm(6)+2*rbm(2)*rbm(1)*pt(2)+-2*rbm(8)*rbm(3)+-2*rbm(5)*rbm(1)+rbm(3)*rbm(3)*pt(1)+2*rbm(3)*rbm(4)*pt(2)-rbm(4)*rbm(4)*pt(1)+2*rbm(4)*rbm(7)+-2*rbm(4)*rbm(1)*pt(3)+rbm(1)*rbm(1)*pt(1);
t(2) = -rbm(2)*rbm(2)*pt(2)+2*rbm(2)*rbm(5)+2*rbm(2)*rbm(4)*pt(3)+-2*rbm(2)*rbm(1)*pt(1)+-2*rbm(8)*rbm(4)-rbm(3)*rbm(3)*pt(2)+2*rbm(3)*rbm(4)*pt(1)+-2*rbm(3)*rbm(7)+2*rbm(3)*rbm(1)*pt(3)+-2*rbm(6)*rbm(1)+rbm(4)*rbm(4)*pt(2)+rbm(1)*rbm(1)*pt(2);
t(3) = rbm(2)*rbm(2)*pt(3)+-2*rbm(2)*rbm(8)+2*rbm(2)*rbm(3)*pt(1)+2*rbm(2)*rbm(4)*pt(2)+-2*rbm(5)*rbm(4)-rbm(3)*rbm(3)*pt(3)+2*rbm(3)*rbm(6)+-2*rbm(3)*rbm(1)*pt(2)-rbm(4)*rbm(4)*pt(3)+2*rbm(4)*rbm(1)*pt(1)+-2*rbm(7)*rbm(1)+rbm(1)*rbm(1)*pt(3);
t(4) = rbm(2)*rbm(2)*pt(4)+-2*rbm(2)*rbm(8)*pt(3)+-2*rbm(2)*rbm(5)*pt(2)+2*rbm(2)*rbm(6)*pt(1)+2*rbm(8)*rbm(8)+-2*rbm(8)*rbm(3)*pt(1)+-2*rbm(8)*rbm(4)*pt(2)+2*rbm(5)*rbm(5)+2*rbm(5)*rbm(4)*pt(3)+-2*rbm(5)*rbm(1)*pt(1)+rbm(3)*rbm(3)*pt(4)+-2*rbm(3)*rbm(6)*pt(3)+2*rbm(3)*rbm(7)*pt(2)+2*rbm(6)*rbm(6)+-2*rbm(6)*rbm(1)*pt(2)+rbm(4)*rbm(4)*pt(4)+-2*rbm(4)*rbm(7)*pt(1)+2*rbm(7)*rbm(7)+-2*rbm(7)*rbm(1)*pt(3)+rbm(1)*rbm(1)*pt(4);



